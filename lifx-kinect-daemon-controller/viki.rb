#!/usr/bin/env ruby
require 'bundler'
Bundler.require
require 'webrick'
require 'pp'
#require 'htmlentities'

c = LIFX::Client.lan
c.discover

# Toujours avoir les bonnes infos
thr = Thread.new do
  t = lastCount = 0
  loop do
    c.lights.refresh
    c.flush
    if c.lights.count > lastCount
      lastCount = c.lights.count
      print "\nLights found: #{c.lights.count}"
    end
    if t > 10
      sleep 60
    else
      sleep 1
    end
    t+=1
    print "."
  end
end
#thr.join

# Main HTML render
def tpl(c)
  rows = []
  html = '<html><head><title>LIFXperience</title></head><body>'
  html += '<table>'
  html += '<thead><tr><th>ID</th><th>Label</th><th>Tags</th><th>Power</th><th>Brightness</th><th>Hue</th><th>Kelvin</th><th>Saturation</th></tr></thead>'
  c.lights.each do |l|
    row = { 'html' => '', 'tag' => l.tags.join(',') }
    row['html'] += '<tr>'
    row['html'] += '<td>'+l.id+'</td>'
    row['html'] += '<td>'+l.label+'</td>'
    row['html'] += '<td>'+l.tags.join(',')+'</td>'
    row['html'] += '<td>'+(l.power==:on ? 'ON' : 'OFF')+' <a href="/'+(l.power==:on ? 'off' : 'on')+'/'+l.id+'">'+(l.power==:on ? 'OFF' : 'ON')+'</a></td>'
    row['html'] += '<td>'+"#{l.color.brightness}"+'</td>'
    row['html'] += '<td>'+"#{l.color.hue}"+'</td>'
    row['html'] += '<td>'+"#{l.color.kelvin}"+'</td>'
    row['html'] += '<td>'+"#{l.color.saturation}"+'</td>'
    row['html'] += '</tr>'
    rows.push(row)
  end
  rows.sort! { |a,b| a['tag'] <=> b['tag'] }
  rows.each do |r|
    html += r['html']
  end
  html += '</table>'
  html += '</body></html>'
  return html
end

server = WEBrick::HTTPServer.new :Port => ARGV.first || 1234
trap('INT') {
  server.stop
  exit
}

server.mount_proc '/' do |req, res|
  path = req.path[1..-1].split('/')
  #pp path

  # Target bulb
  begin
    case path[0]
    when "refresh"
      c.lights.refresh
    when "add_tag"
      c.lights.with_id(path[1]).add_tag(path[2])
    when "remove_tag"
      c.lights.with_id(path[1]).remove_tag(path[2])
    else
      case path[1]
      when "all"
        target = c.lights
        path.delete_at(1)
      when "label"
        target = c.lights.with_label(path[2])
        path.delete_at(1)
        path.delete_at(1)
      when "tag"
        target = c.lights.with_tag(path[2])
        path.delete_at(1)
        path.delete_at(1)
      else
        target = c.lights.with_id(path[1])
        path.delete_at(1)
      end

      # Path is clean from target idents, only action args left
      if target
        if path[0]=="on" || path[0]=="off"
          target.send('turn_'+path[0])
          c.flush
        elsif path[0]=="color"
          speed = 0.5
          if path[2]=="slow"
            speed = 5
          end
          if path[1]=="white"
            col = LIFX::Color.white(brightness: 1.0, kelvin: 10000)
          elsif path[1]=="random"
              col = LIFX::Color.random_color()
          else
            sat = 1;
            bright = 1;
            if path[1][0,4]=="dark"
              bright = 0.1
              path[1] = path[1][4..-1]
            elsif path[1][0,5]=="light"
              sat = 0.2
              bright = 0.8
              path[1] = path[1][5..-1]
            end
            puts path[1]
            #if LIFX::Color.method_defined?(path[1])
              col = LIFX::Color.send(path[1], saturation: sat, brightness: bright, kelvin: 10000);
            #end
          end
          if col
            target.set_power(:on).send('set_color',col, duration:speed)
            c.flush
          end
        end
     end
   end
  rescue => detail
    puts detail
  end

  res.status = 200
  res['Content-Type'] = 'text/html; charset=utf-8'
  res.body = tpl(c)
end

server.start